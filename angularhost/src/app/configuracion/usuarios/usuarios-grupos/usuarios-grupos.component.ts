import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'app-usuarios-grupos',
  templateUrl: './usuarios-grupos.component.html',
  styleUrls: ['./usuarios-grupos.component.scss']
})
export class UsuariosGruposComponent implements OnInit {
  @Input('groupsLst') resultLst: string;
  @Input('editQuery') editQuery: number;
  @Input('id') id: string;
  disabled = true;

  constructor() { 
  }

  ngOnInit() {
    if (this.id == "0" || this.editQuery == 1) {
      this.disabled = false;
    }
  }


}
