﻿using Eleon.Modding;
using EmpyrionModWebHost.Extensions;
using EmpyrionModWebHost.Models;
using EmpyrionNetAPIAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EmpyrionModWebHost.Controllers
{

    public class GlobalStructureListBackup
    {
        public Tuple<string, GlobalStructureInfo[]>[] Structures { get; set; }
    }

    public class StructureManager : EmpyrionModBase, IEWAPlugin, IDisposable
    {

        public ModGameAPI GameAPI { get; private set; }
        public ConfigurationManager<GlobalStructureList> LastGlobalStructureList { get; private set; }
        public string CurrentEBPFile { get; set; }

        public StructureManager()
        {
            LastGlobalStructureList = new ConfigurationManager<GlobalStructureList>()
            {
                UseJSON = true,
                ConfigFilename = Path.Combine(EmpyrionConfiguration.SaveGameModPath, "DB", "GlobalStructureList.json")
            };
            LastGlobalStructureList.Load();
        }

        public void Dispose()
        {
            try { System.IO.File.Delete(CurrentEBPFile); } catch { }
        }

        public override void Initialize(ModGameAPI dediAPI)
        {
            GameAPI = dediAPI;

            API_Exit += () => { try { System.IO.File.Delete(CurrentEBPFile); } catch { } };

            TaskTools.Intervall(10000, () => GlobalStructureList());
        }


        public GlobalStructureList GlobalStructureList()
        {
            try
            {
                LastGlobalStructureList.Current = Request_GlobalStructure_List().Result;
                TaskTools.Delay(0, () => LastGlobalStructureList.Save());
            }
            catch { }

            return LastGlobalStructureList.Current;
        }

        public async Task CreateStructureAsync(string currentEBPFile, PlayfieldGlobalStructureInfo aStructure)
        {
            var NewID = await Request_NewEntityId();

            //aStructure.Type = "CV";

            var TargetDir = Path.Combine(
                EmpyrionConfiguration.ProgramPath,
                "Content", "Scenarios",
                EmpyrionConfiguration.DedicatedYaml.CustomScenarioName,
                "Prefabs");

            Directory.CreateDirectory(Path.GetDirectoryName(TargetDir));
            var TargetFilename = Path.Combine(TargetDir, Path.GetFileName(CurrentEBPFile));
            File.Copy(CurrentEBPFile, TargetFilename, true);

            var SpawnInfo = new EntitySpawnInfo()
            {
                forceEntityId = NewID.id,
                playfield = aStructure.Playfield,
                pos = aStructure.Pos,
                rot = aStructure.Rot,
                name = $"EBP:{Path.GetFileNameWithoutExtension(aStructure.Name)}",
                type = (byte)Array.IndexOf(new[] { "Undef", "", "BA", "CV", "SV", "HV", "", "AstVoxel" }, aStructure.Type), // Entity.GetFromEntityType 'Kommentare der Devs: Set this Undef = 0, BA = 2, CV = 3, SV = 4, HV = 5, AstVoxel = 7
                entityTypeName = "", // 'Kommentare der Devs:  ...or set this to f.e. 'ZiraxMale', 'AlienCivilian1Fat', etc
                prefabName = Path.GetFileNameWithoutExtension(CurrentEBPFile),
                prefabDir  = Path.GetDirectoryName(CurrentEBPFile),
                factionGroup = 0,
                factionId = 0, // erstmal auf "public" aStructure.Faction,
            };

            try { await Request_Load_Playfield(new PlayfieldLoad(20, aStructure.Playfield, 0)); }
            catch { }  // Playfield already loaded

            try
            {
                await Request_Entity_Spawn(SpawnInfo);
                await Request_Structure_Touch(NewID); // Sonst wird die Struktur sofort wieder gelöscht !!!
            }
            finally
            {
                try { File.Delete(TargetFilename); } catch { }
            }
        }

    }

    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class StructureController : ControllerBase
    {
        public StructureManager StructureManager { get; }

        public StructureController()
        {
            StructureManager = Program.GetManager<StructureManager>();
        }

        [HttpGet("GlobalStructureList")]
        public IActionResult GlobalStructureList()
        {
            return Ok(StructureManager.GlobalStructureList());
        }

        public class DeleteStructuresData
        {
            public int id { get; set; }
            public string playfield { get; set; }
        }

        [HttpPost("DeleteStructures")]
        public IActionResult DeleteStructures([FromBody]DeleteStructuresData[] aEntites)
        {
            aEntites
                .OrderBy(E => E.playfield)
                .ForEach(I =>
                {
                    try
                    {
                        StructureManager.Request_Load_Playfield(new PlayfieldLoad(20, I.playfield, 0)).Wait();
                        Thread.Sleep(2000); // wait for Playfield finish
                    }
                    catch { }  // Playfield already loaded
                    StructureManager.Request_Entity_Destroy(new Id(I.id));
                });
            return Ok();
        }

        public class SetFactionOfStucturesData
        {
            public string FactionAbbrev { get; set; }
            public int[] EntityIds { get; set; }
        }

        [HttpPost("SetFactionOfStuctures")]
        public IActionResult SetFactionOfStuctures([FromBody]SetFactionOfStucturesData aData)
        {
            aData.EntityIds.ForEach(I => StructureManager.Request_ConsoleCommand(new PString($"faction entity '{aData.FactionAbbrev}' {I}")));
            return Ok();
        }

        [HttpPost("UploadEBPFile")]
        [DisableRequestSizeLimit]
        public IActionResult UploadEBPFile()
        {
            Program.CreateTempPath();

            foreach (var file in Request.Form.Files)
            {
                try { System.IO.File.Delete(StructureManager.CurrentEBPFile); } catch { }
                StructureManager.CurrentEBPFile = System.IO.Path.GetTempPath() + file.FileName;

                try { Directory.CreateDirectory(Path.GetDirectoryName(StructureManager.CurrentEBPFile)); } catch { }

                using (var ToFile = System.IO.File.Create(StructureManager.CurrentEBPFile))
                {
                    file.OpenReadStream().CopyTo(ToFile);
                }
            }
            return Ok();
        }

        [HttpPost("CreateStructure")]
        public IActionResult CreateStructure([FromBody]PlayfieldGlobalStructureInfo aData)
        {
            StructureManager.CreateStructureAsync(StructureManager.CurrentEBPFile, aData).Wait();
            return Ok();
        }

    }
}
