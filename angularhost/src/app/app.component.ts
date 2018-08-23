import { Component, OnInit } from '@angular/core';
import { UpdateService } from './services/update.service';
import { HubConnection } from '@aspnet/signalr';
import * as signalR from '@aspnet/signalr';
import { ConfigService } from './services/config.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  title = 'app';
  private _hubConnection: HubConnection;
  msgs = [];

  constructor(private update: UpdateService, private config : ConfigService) {
    //update.checkForUpdate();
  }

  ngOnInit(): void {
    this._hubConnection = new signalR.HubConnectionBuilder()
        .withUrl(this.config.BASE_SIGNALR)
        .configureLogging(signalR.LogLevel.Information)
        .build();

    this._hubConnection
      .start()
      .then(() => console.log('Conectado a mensajeria! :)'))
      .catch(err => console.log('Error conectando a mensajeria:('));
  
    this._hubConnection.on('Send', (type: string, payload: string) => {
      this.msgs.push({ severity: type, summary: payload });
    });
  }
}
