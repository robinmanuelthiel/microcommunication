import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class RandomService {

  constructor(private httpClient: HttpClient) { }

  async getRandomDice(): Promise<number> {
    return this.httpClient.get<number>(environment.apiUrl + '/api/random/dice', {
      headers: new HttpHeaders().set('ApiKey', environment.apiKey)
    }).toPromise();
  }

  async getRandomValue(): Promise<number> {
    return this.httpClient.get<number>(environment.apiUrl + '/api/random/value', {
      headers: new HttpHeaders().set('ApiKey', environment.apiKey)
    }).toPromise();
  }
}
