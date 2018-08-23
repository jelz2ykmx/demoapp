import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { FormGroup, FormControl } from '@angular/forms';

@Component({
  selector: 'app-menu-filtro',
  templateUrl: './menu-filtro.component.html',
  styleUrls: ['./menu-filtro.component.scss']
})
export class MenuFiltroComponent implements OnInit {
  @Input('status') typeactivoinactivo: number;
  @Output() onToggleFiltro = new EventEmitter<void>();
  @Output() onGoSearch = new EventEmitter<number>();
  
  title = 'Filtro';

  showResults = true;
  loading: boolean = false;
  
  formData = {};
  basicForm: FormGroup;

  typeArray = [
      { id : 0,  typeName : 'Inactivos' },
      { id : 1, typeName : 'Activos' },
      { id : 2, typeName : 'Todos' },
  ];

  constructor() {
    this.basicForm = new FormGroup({
        typeactivoinactivo: new FormControl('',  [ ]),
    });
  }
 
  ngOnInit() {
    this.basicForm.controls["typeactivoinactivo"].setValue(this.typeactivoinactivo);
  }

  doConsulta() {
    this.onToggleFiltro.emit();
  }

  reloadQuery() {
      this.onGoSearch.emit(this.basicForm.controls["typeactivoinactivo"].value);
  }
}
