import { Injectable } from '@angular/core';
@Injectable({
  providedIn: 'root'
})
export class HomeService  {

  public jumbotronEnabled:boolean=true;

  constructor() { }

  enableJumbotron(value:boolean){
    this.jumbotronEnabled=value;
  }
}
