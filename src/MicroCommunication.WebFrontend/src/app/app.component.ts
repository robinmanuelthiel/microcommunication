import { Component, OnInit } from '@angular/core';
import { RandomService } from './services/random.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.sass']
})
export class AppComponent implements OnInit {
  title = 'MicroCommunication-Web';
  dice = 6;
  imagePath = 'assets/images/dice-' + this.dice + '.svg';

  constructor(private randomService: RandomService) { }

  async ngOnInit() {
    this.dice = await this.randomService.getRandomDice();
  }

  async rollDiceAsync(): Promise<void> {
    this.dice = await this.randomService.getRandomDice();
  }
}
