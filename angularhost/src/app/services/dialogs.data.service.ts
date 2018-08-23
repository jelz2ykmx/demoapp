import { Injectable } from '@angular/core';
import { MatDialog } from '@angular/material';
import { WebservicesService } from './webservices.service';
import { ModalspinnerComponent } from '../shared/modalspinner/modalspinner.component';
import { ModalsaveComponent } from '../shared/modalsave/modalsave.component';
import { DialogSnackService } from './dialog.snack.service';

@Injectable({
  providedIn: 'root'
})
export class DialogsDataService {

  constructor(public dialog: MatDialog, private webservices: WebservicesService, private snack : DialogSnackService ) { }

  createView(view) {
    if (view == 0) {
      view = ModalsaveComponent;
    }
    let dialogRef = this.dialog.open(view,  {
      width: '265px',
      disableClose: true,
      panelClass: 'spinner-dialog'
      //data: { name: this.name, animal: this.animal }
    });
    return dialogRef;
  }

  runWebservices(path, model, type) : any {
    let dialogRef = this.createView(ModalspinnerComponent);
    return this.webservices.postMessage(path, model)
    .then( data => {
      dialogRef.close();
      if (data == null) {
        if (type == 0) {
          this.snack.showSnack("Registro ha sido gurdado con exito ");
        }
      }
      return data;
    }).catch( err => {
      dialogRef.close();
    });
  }
  
 

  checkError(newForm) {
    if (!newForm.valid) {
      for (var control in  newForm.controls) {
        if (newForm.controls[control].invalid) {
          if (newForm.controls[control]['tagname'] != undefined) {
            control = newForm.controls[control]['tagname'];
          }
          this.snack.showSnack("Debe ingresar un valor valido para el campo " + control);
          return true;
        }
      }
    }
    return false;
  }
}
