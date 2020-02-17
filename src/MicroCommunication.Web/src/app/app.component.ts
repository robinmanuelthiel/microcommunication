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
  isError = false;
  errorMessage: string;

  constructor(private randomService: RandomService) { }

  async ngOnInit() {
    await this.rollDiceAsync();
  }

  async rollDiceAsync(): Promise<void> {
    try {
      this.isError = false;
      this.dice = await this.randomService.getRandomDice();
      this.imagePath = 'assets/images/dice-' + this.dice + '.svg';
    } catch (error) {
      this.isError = true;
      this.errorMessage = error.message;
    }
  }
}
