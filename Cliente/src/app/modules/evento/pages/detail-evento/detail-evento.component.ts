import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Params } from '@angular/router';
import { Location } from '@angular/common';

@Component({
  selector: 'app-detail-evento',
  templateUrl: './detail-evento.component.html',
  styleUrls: ['./detail-evento.component.css']
})
export class DetailEventoComponent implements OnInit {

  public objEvento: any;
  constructor(
    private actRouted: ActivatedRoute,
    public location: Location
  ) { }

  ngOnInit(): void {
    history.pushState(null, '');
    window.scroll(0, 0);
    this.actRouted.params.subscribe(
      (params: Params) => {
        this.objEvento = JSON.parse(atob(params.data));
      }
    );
  }

  backPage(){
    this.location.back()
  }

}
