import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { FormGroup, FormControl } from '@angular/forms';

@Component({
  selector: 'app-companies-filtro',
  templateUrl: './companies-filtro.component.html',
  styleUrls: ['./companies-filtro.component.scss']
})
export class CompaniesFiltroComponent implements OnInit {
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
