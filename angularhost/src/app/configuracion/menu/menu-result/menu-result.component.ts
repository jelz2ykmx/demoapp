import { Component, Input, Output, EventEmitter } from '@angular/core';
import { DialogsDataService } from '../../../services/dialogs.data.service';

@Component({
  selector: 'app-menu-result',
  templateUrl: './menu-result.component.html',
  styleUrls: ['./menu-result.component.scss']
})
export class MenuResultComponent {
  @Input('resultLst') resultLst: any;
  @Input('path') path: string;
  @Output() onEditQuery = new EventEmitter<any>();
  @Input('status') status: number;
  
  isDelete = false;
  isUnLock = false;

  constructor(private dialogsService : DialogsDataService) { }

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
