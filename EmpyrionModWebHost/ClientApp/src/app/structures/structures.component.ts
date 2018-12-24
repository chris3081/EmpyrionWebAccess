import { Component, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource, MatSort, MatPaginator } from '@angular/material';
import { HttpClient } from '@angular/common/http';
import { StructureService } from '../services/structure.service';
import { Router } from '@angular/router';
import { GlobalStructureInfo } from '../model/structure-model';
import { PositionService } from '../services/position.service';
import { SelectionModel } from '@angular/cdk/collections';
import { FactionService } from '../services/faction.service';
import { YesNoDialogComponent, YesNoData } from '../yes-no-dialog/yes-no-dialog.component';
import { PlayerService } from '../services/player.service';
import { FactionSelectDialogComponent } from '../faction-select-dialog/faction-select-dialog.component';

@Component({
  selector: 'app-structures',
  templateUrl: './structures.component.html',
  styleUrls: ['./structures.component.less']
})
export class StructuresComponent implements OnInit {
  @ViewChild(YesNoDialogComponent) YesNo: YesNoDialogComponent;
  @ViewChild(FactionSelectDialogComponent) FactionSelect: FactionSelectDialogComponent;

  displayedColumns = ['Select', 'Id', 'Playfield', 'Name', 'TypeName', 'CoreName', 'FactionName', 'PosX', 'PosY', 'PosZ', 'RotX', 'RotY', 'RotZ', 'dockedShips', 'classNr', 'cntLights', 'cntTriangles', 'cntBlocks', 'cntDevices', 'fuel', 'powered', 'factionGroup', 'pilotId'];
  structures: MatTableDataSource<GlobalStructureInfo> = new MatTableDataSource([]);

  selection = new SelectionModel<GlobalStructureInfo>(true, []);

  @ViewChild(MatSort) sort: MatSort;
  @ViewChild(MatPaginator) paginator: MatPaginator;
    error: any;

  constructor(
    public router: Router,
    private http: HttpClient,
    public PlayerService: PlayerService,
    private mStructureService: StructureService,
    private mPositionService: PositionService,
    public FactionService: FactionService,
  ) { }

  ngOnInit() {
    this.mStructureService.GetGlobalStructureList().subscribe(S => {
      this.structures.data = S.map((s:any) => {
        s.CoreName = ["None", "Player", "Admin", "Alien", "AlienAdmin"][s.coreType];
        s.TypeName = ["Undef", "", "BA", "CV", "SV", "HV", "", "AstVoxel"][s.type];
        let Faction = this.FactionService.GetFaction(s.factionId);
        s.FactionName = Faction ? Faction.Abbrev : "";
        return s;
      });
    });
  }

  ngAfterViewInit() {
    this.structures.sort = this.sort;
    this.structures.paginator = this.paginator;
  }

  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.structures.filter = filterValue;
  }

  /** Whether the number of selected elements matches the total number of rows. */
  isAllSelected() {
    const numSelected = this.selection.selected.length;
    const numRows = this.structures.data.length;
    return numSelected == numRows;
  }

  /** Selects all rows if they are not all selected; otherwise clear selection. */
  masterToggle() {
    this.isAllSelected() ?
      this.selection.clear() :
      this.structures.data.forEach(row => this.selection.select(row));
  }

  select(row : GlobalStructureInfo) {
    this.selection.clear();
    this.selection.toggle(row);
  }

  SavePosition(aStruct: GlobalStructureInfo) {
    this.mPositionService.CurrentPosition = { description: "Structure: " + aStruct.name, playfield: aStruct.playfield, entityId: aStruct.id, pos: aStruct.pos, rot: aStruct.rot };
  }

  ReloadStructures() {
    this.mStructureService.ReloadStructures();
  }

  Destroy() {
    this.YesNo.openDialog({ title: "Destroy", question: this.selection.selected.length + " structures?" }).afterClosed().subscribe(
      (YesNoData: YesNoData) => {
        if (!YesNoData.result) return;
        this.http.post<number[]>("Structure/DeleteStructures", this.selection.selected.map(S => S.id))
          .pipe()
          .subscribe(
            S => {},
            error => this.error = error // error path
          );
      });
  }

  ChangeFaction() {
    this.FactionSelect.openDialog("Set faction of " + this.selection.selected.length + " structures?").afterClosed().subscribe(
      (SelectedFaction: string) => {
        if (!SelectedFaction) return;
        this.http.post("Structure/SetFactionOfStuctures", { FactionAbbrev: SelectedFaction, EntityIds: this.selection.selected.map(S => S.id) })
          .pipe()
          .subscribe(
            S => { },
            error => this.error = error // error path
          );
      });
  }
}
