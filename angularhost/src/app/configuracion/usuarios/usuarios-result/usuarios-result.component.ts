import { Component, Input, EventEmitter, Output } from '@angular/core';
import { DialogsDataService } from '../../../services/dialogs.data.service';
import { MenuItemsService } from '../../../services/menu.items.service';

@Component({
  selector: 'app-usuarios-result',
  templateUrl: './usuarios-result.component.html',
  styleUrls: ['./usuarios-result.component.scss']
})
export class UsuariosResultComponent {
  routingPath = 'Configuracion/Usuarios';
  @Input('resultLst') resultLst: any;
  @Input('path') path: string; 
  @Output() onEditQuery = new EventEmitter<any>();
  @Input('status') status: number;
  
  isDelete = false;
  isUnLock = false;
  isedit = false;
  isdelete = false;
  
  constructor(private dialogsService : DialogsDataService, private menurights : MenuItemsService) { 
    if (menurights.menuItemsRights[this.routingPath].isdelete == 1) {
      this.isdelete = true;
    }
    if (menurights.menuItemsRights[this.routingPath].isedit == 1) {
      this.isedit = true;
    }
  }

  edicionConsulta(idValue, typeValue, email) {
    var model = { 
      id : idValue, 
      editQuery : typeValue
    };
    if (typeValue == 2) {
      model["email"] = email;
    }
    
    this.onEditQuery.emit(model);
   
  }

  isGotoDelete() {
    this.isDelete = true;
  }

  delete(id) {
    this.lockUnlock(1, id);
  }

  lockUnlock(status, id) {
    var model = {
      type : 5,
      isActive : {
        id : id,
        status : status
      }
    };
    this.dialogsService.runWebservices(this.path, model, 1)
    .then( data => {
      if (data == null) {
        for (var x = 0; x < this.resultLst.length; x++) {
          var tmp = this.resultLst[x] as any;
          if (tmp.id == model.isActive.id) {
            if (this.status != 2) {
              this.resultLst.splice(x, 1);
            } 
            else {
              this.resultLst[x].status = model.isActive.status;
              this.isUnLock = false;
            }  
            break;
          } 
        }
        this.isDelete = false;
      }
    });
  }

  undo() {
    this.isDelete = false;
  }

  undoUnlock() {
    this.isDelete = true;
    this.isUnLock = false;
  }

  unlock(res) {
    if (!this.isUnLock) {
      this.isDelete = false;
      this.isUnLock = true;
    }
    else {
      this.lockUnlock(0, res)
    }
    
  }

}
