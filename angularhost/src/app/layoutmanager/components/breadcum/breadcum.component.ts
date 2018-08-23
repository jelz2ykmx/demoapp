import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'app-breadcum',
  templateUrl: './breadcum.component.html',
  styleUrls: ['./breadcum.component.scss']
})
export class BreadcumComponent implements OnInit {
  @Input('breadcum') breadcum: string;
  
  constructor() { }

  ngOnInit() {
      
  }

}
