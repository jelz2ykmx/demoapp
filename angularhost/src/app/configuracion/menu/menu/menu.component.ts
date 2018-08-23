import { Component, ViewChild } from '@angular/core';
import { FormBuilder } from '@angular/forms';
//import { MenuEditComponent } from '../menu-edit/menu-edit.component';
import { DialogsDataService } from '../../../services/dialogs.data.service';

@Component({
  selector: 'app-menu',
  templateUrl: './menu.component.html',
  styleUrls: ['./menu.component.scss']
})
export class MenuComponent {
  //@ViewChild(MenuEditComponent)
  //private editQueryChild: MenuEditComponent;

  path = "api/menu";
  
  basicForm;

  title = "Búsqueda";

  filter = "";
  status = 1;
  resultLst = [];

  search = true;
  edit = false;
  viewFilter = false;

  id = '0';
  editQuery = 0;
  
  constructor(private fb: FormBuilder, private dialogsService : DialogsDataService) { 
    this.basicForm = fb.group ({
      filter: ["",[] ],
    });
  }

  /*
  onChanges(): void {
  this.basicForm.get('filter').valueChanges.subscribe(val => {
    this.filter = val;
  });
  }
  */

  doSearch() {
    this.resultLst = [];
    var model = {
      type : 1,
      search : {
        filter : this.basicForm.get('filter').value,
        status : this.status
      }
    }

    this.dialogsService.runWebservices(this.path, model, 1)
    .then( data => {
      if (data.error == null) {
        this.resultLst = data;
      }
    });
  }

  doNuevo() {
    this.id = '0';
    this.search = false;
    this.edit = true;
  }


  onSearch(model) {
    if (model.typeOperation == 0) {
      var tmpmodel = {
        id : model.id,
        menu : model.menu,
        status : 1
      }  
      this.resultLst.splice(0, 0, tmpmodel);
    }   
    else if (model.typeOperation == 1) {
      for (var x = 0; x < this.resultLst.length; x++) {
        var tmp = this.resultLst[x] as any;
        if (tmp.id == model.id) {
            this.resultLst[x].menu = model.menu;
        } 
      }  
    }
    this.edit = false;
    this.search = true;
  }

  onEditQuery(model) {
    this.id = model.id;
    this.editQuery = model.editQuery;
    this.search = false;
    this.edit = true;
  }

  onGoSearch(status) {
    this.onToggleFiltro();
    this.status = status;
    this.doSearch();
  }

  toggleFiltro() {
    this.search = false;
    this.viewFilter = true;
  }

  onToggleFiltro() {
    this.search = true;
    this.viewFilter = false;
  }

}
