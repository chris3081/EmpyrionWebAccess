import { Component, OnInit, Output, ViewChild } from '@angular/core';
import { PlayerService } from '../services/player.service';
import { PlayerModel } from '../model/player-model';
import { PlayfieldService } from '../services/playfield.service';
import { FactionService } from '../services/faction.service';
import { FactionModel } from '../model/faction-model';
import { MatMenu, MatMenuTrigger } from '@angular/material';
import { YesNoDialogComponent, YesNoData } from '../yes-no-dialog/yes-no-dialog.component';
import { PlayfieldModel } from '../model/playfield-model';

@Component({
  selector: 'app-player-details',
  templateUrl: './player-details.component.html',
  styleUrls: ['./player-details.component.less']
})
export class PlayerDetailsComponent implements OnInit {
  @ViewChild(YesNoDialogComponent) YesNo: YesNoDialogComponent;
  Player: PlayerModel;
  Playfields: PlayfieldModel[];
  Factions: FactionModel[];
  @Output() Changed: boolean;
  @ViewChild(MatMenu) contextMenu: MatMenu;
  @ViewChild(MatMenuTrigger) contextMenuTrigger: MatMenuTrigger;

  constructor(
    private mPlayfields: PlayfieldService,
    private mPlayerService: PlayerService,
    private mFactionService: FactionService
  ) {
    this.SyncPlayer(this.mPlayerService.CurrentPlayer);
    mPlayerService.GetCurrentPlayer().subscribe(P => this.SyncPlayer(P));
    mFactionService.GetFactions().subscribe(F => this.Factions = F);
  }

  SyncPlayer(aPlayer: PlayerModel) {
    if (this.Changed || !aPlayer) return;

    this.Player = JSON.parse(JSON.stringify(aPlayer));
    this.Player.Food = Math.floor(this.Player.Food);
  }

  ngOnInit() {
    this.mPlayfields.PlayfieldNames.subscribe(PL => this.Playfields = PL);
  }

  SaveChanges() {
    this.contextMenuTrigger.closeMenu();
    if (this.Player.FactionId) this.Player.FactionGroup = 0;  // gehört einer Faction an
    this.mPlayerService.saveUser(this.Player);
    this.Changed = false;
  }

  DiscardChanges() {
    this.Changed = false;
    this.SyncPlayer(this.mPlayerService.CurrentPlayer);
  }

  Ban(aPlayer: PlayerModel) {
    this.contextMenuTrigger.closeMenu();
    this.YesNo.openDialog({ title: "Ban player", question: aPlayer.PlayerName }).afterClosed().subscribe(
      (YesNoData: YesNoData) => {
        if (!YesNoData.result) return;
        this.mPlayerService.BanPlayer(this.Player);
      });
  }

  UnBan(aPlayer: PlayerModel) {
    this.contextMenuTrigger.closeMenu();
    this.mPlayerService.UnBanPlayer(this.Player);
  }

  Wipe(aPlayer: PlayerModel) {
    this.contextMenuTrigger.closeMenu();
    this.YesNo.openDialog({ title: "Wipe player", question: aPlayer.PlayerName }).afterClosed().subscribe(
      (YesNoData: YesNoData) => {
        if (!YesNoData.result) return;
        this.mPlayerService.WipePlayer(this.Player);
      });
  }

  PlayerHint(aPlayer: PlayerModel) {
    let FoundElevated = this.mPlayerService.ElevatedUser.find(U => U.steamId == aPlayer.SteamId);
    if (FoundElevated) switch (FoundElevated.permission) {
      case 3: return "GameMaster";
      case 6: return "Moderator";
      case 9: return "Admin";
    }

    let FoundBanned = this.mPlayerService.BannedUser.find(U => U.steamId == aPlayer.SteamId);
    if (FoundBanned) return "Banned until " + FoundBanned.until.toLocaleString();

    return "";
  }

}
