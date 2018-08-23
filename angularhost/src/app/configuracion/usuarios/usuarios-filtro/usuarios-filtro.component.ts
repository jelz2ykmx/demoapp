import { Component, Input, EventEmitter, Output, OnInit } from '@angular/core';
import { FormGroup, FormControl } from '@angular/forms';


@Component({
  selector: 'app-usuarios-filtro',
  templateUrl: './usuarios-filtro.component.html',
  styleUrls: ['./usuarios-filtro.component.scss']
})
export class UsuariosFiltroComponent implements OnInit {
  @Input('status') typeactivoinactivo: number;
  @Output() onToggleFiltro = new EventEmitter<void>();
  @Output() onGoSearch = new EventEmitter<number>();
  
  title = 'Filtro';

  showResults = true;
  loading: boolean = false;
  
  formData = {};
  basicForm: FormGroup;

  typeArray = [
      { id : 1,  typeName : 'Inactivos' },
      { id : 0, typeName : 'Activos' },
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
