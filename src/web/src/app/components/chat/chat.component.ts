import { Component, OnInit, OnDestroy } from '@angular/core';
import { HubConnectionBuilder, HubConnection } from '@microsoft/signalr';
import { environment } from 'src/environments/environment';
import { ChatMessage } from 'src/models/chatMessage';

@Component({
  selector: 'app-chat',
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.sass']
})
export class ChatComponent implements OnInit, OnDestroy {
  private streamConnection: HubConnection;
  serverName = '';
  messages = [];
  name = 'Robin';

  constructor() { }

  async ngOnInit() {
    this.streamConnection = new HubConnectionBuilder()
      .withUrl(`${environment.apiUrl}/chat`)
      .withAutomaticReconnect([0, 3000, 3000, 3000, 3000, 3000, 3000, 3000, 3000, 3000, 3000])
      .build();

    this.streamConnection.on('Name', (name: string) => this.serverName = name);
    this.streamConnection.on('ReceiveMessage', (message: ChatMessage) => this.messages.push(message));
    await this.streamConnection.start();
  }

  async ngOnDestroy() {
    await this.streamConnection.stop();
  }

  async sendMessage(message: string) {
    if (message) {
      await this.streamConnection.send('SendMessage', message, name);
    }
  }
}
