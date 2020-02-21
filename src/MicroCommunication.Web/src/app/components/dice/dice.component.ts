import { Component, OnInit } from '@angular/core';
import { RandomService } from 'src/app/services/random.service';

@Component({
  selector: 'app-dice',
  templateUrl: './dice.component.html',
  styleUrls: ['./dice.component.sass']
})
export class DiceComponent implements OnInit {

  dice = 5;
  imagePath = '/assets/images/dice-' + this.dice + '.svg';
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
      this.imagePath = '/assets/images/dice-' + this.dice + '.svg';
    } catch (error) {
      this.isError = true;
      this.errorMessage = error.message;
    }
  }
}
