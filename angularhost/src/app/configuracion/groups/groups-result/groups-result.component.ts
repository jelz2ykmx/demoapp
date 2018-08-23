import { Component, Input, Output, EventEmitter } from '@angular/core';
import { DialogsDataService } from '../../../services/dialogs.data.service';
import { MenuItemsService } from '../../../services/menu.items.service';

@Component({
  selector: 'app-groups-result',
  templateUrl: './groups-result.component.html',
  styleUrls: ['./groups-result.component.scss']
})
export class GroupsResultComponent {
  routingPath = 'Configuracion/Grupos';

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

  edicionConsulta(idValue, typeValue) {
    this.onEditQuery.emit({ id : idValue, editQuery : typeValue});
  }

  isGotoDelete(id) {
    this.isDelete = true;
  }

  delete(id) {
    this.lockUnlock(id, 0);
  }

  lockUnlock(id, status) {
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
    this.isUnLock = false;
  }

  unlock(id) {
    if (!this.isUnLock) {
      this.isUnLock = true;
    }
    else {
      this.lockUnlock(id, 1)
    }
  }

}
