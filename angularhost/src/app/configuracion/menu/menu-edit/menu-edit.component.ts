import { Component, Input, Output, EventEmitter } from '@angular/core';
import { FormBuilder, FormControl, Validators } from '@angular/forms';
import { CharacterLimit, fuuidv4 } from '../../../helpers/text-helpers';
import { DialogsDataService } from '../../../services/dialogs.data.service';
import { MenuRole } from '../../../classes/menu.role';
import { DialogSnackService } from '../../../services/dialog.snack.service';

@Component({
  selector: 'app-menu-edit',
  templateUrl: './menu-edit.component.html',
  styleUrls: ['./menu-edit.component.scss']
})
export class MenuEditComponent {
  newMenuForm;
  title = "Nuevo";
  readonly = false;
  typeOperation = 0;
  interval;
  
  @Input('id') id: string;
  @Input('path') path: string; 
  @Input('editQuery') editQuery: number;
  @Output() onSearch = new EventEmitter<any>();

  constructor(private fb: FormBuilder, private dialogsService : DialogsDataService, private snack : DialogSnackService ) { 
    let menuRoleClass = new MenuRole();
   
    this.newMenuForm = this.fb.group ({
      menu : new FormControl('', [ Validators.required, CharacterLimit(256),  menuRoleClass.validFormat() ] ),
      entitie : new FormControl('', [ Validators.required, CharacterLimit(256) ] ),
    });
    this.newMenuForm.get('entitie')['tagname'] = 'entidad';
    
    this.interval = setInterval( () => { 
      clearInterval(this.interval);
      if (this.id != "0") {
        if (this.editQuery == 1) {
          this.title = "Editar";
          this.typeOperation = 1;
        }
        else {
          this.title = "Consulta";
          this.typeOperation = 2;
        }
        this.getById();
      }
    });
  }

  isValid(control) {
    return this.newMenuForm.controls[control].invalid && this.newMenuForm.controls[control].touched;
  }

  getById() {
    var model = {
      type : 2,
      byId : {
        id : this.id,
      }
    }

    this.dialogsService.runWebservices(this.path, model, 1)
    .then( data => {
      if (data != null ) {
        if (data.menu != undefined) {
          this.newMenuForm.controls['menu'].setValue(data.menu);
          this.newMenuForm.controls['entitie'].setValue(data.entitie);
          if (this.editQuery == 0) {
            this.newMenuForm.controls['menu'].disable();
            this.newMenuForm.controls['entitie'].disable();
            this.readonly = true;
          }
          else { 
            this.readonly = false;
          }
        }
        else {
          this.readonly = true;
        }
      }
    });
  }

  showConfirmacion() {
    if (!this.dialogsService.checkError(this.newMenuForm)) {
      let dialogRef = this.dialogsService.createView(0);
      dialogRef.afterClosed().subscribe(result => {
        if (result != 0) {
          if (result == 1) {
            this.doUpdate();
          }
        }
        else {
          this.doConsulta(undefined);
        }
      });
    }
  }

  riseError(control) {
    if (this.newMenuForm.controls[control].invalid) {
        this.snack.showSnack("Debe ingresar un valor valido para el campo " + control);
        return true;
    }
    return false;
  }

  doUpdate() {
    var model;
    if (this.id == '0') {
      model = this.CreateUpdateModel(fuuidv4(), 3);
    }
    else {
      model = this.CreateUpdateModel(this.id, 4);
    }
    this.dialogsService.runWebservices(this.path, model, 0)
    .then( data => {
      if (data == null) {
        this.doConsulta(model.update.id);
      }
    });
  }

  CreateUpdateModel(id, type) {
    var model = {
      type : type,
      update : {
        id : id,
        menu : this.newMenuForm.get('menu').value,
        entitie : this.newMenuForm.get('entitie').value,
      }
    };
    return model;
  }

  doConsulta(id) {
    if (id == undefined) {
      this.typeOperation = -1;
    }
    var model = {
      id : id,
      menu : this.newMenuForm.get('menu').value,
      typeOperation : this.typeOperation
    }
    this.onSearch.emit(model);
  }
}
