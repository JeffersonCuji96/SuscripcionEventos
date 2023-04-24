import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-lista-suscripciones',
  templateUrl: './lista-suscripciones.component.html',
  styleUrls: ['./lista-suscripciones.component.css']
})
export class ListaSuscripcionesComponent implements OnInit {

  public title = "EVENTOS SUSCRITOS";
  public suscription = true;

  constructor() { }
  ngOnInit(): void {}
}
