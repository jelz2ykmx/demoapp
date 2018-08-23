import { Component, Input } from '@angular/core';
import { OnInit } from '@angular/core';

@Component({
  selector: 'app-groups-menu',
  templateUrl: './groups-menu.component.html',
  styleUrls: ['./groups-menu.component.scss']
})
export class GroupsMenuComponent implements OnInit {
  @Input('menuLst') resultLst: string;
  @Input('editQuery') editQuery: number;
  @Input('id') id: string;
  flagCursor = true;

  constructor() {
  }

  ngOnInit() {
    if (this.id == "0" || this.editQuery == 1) {
      this.flagCursor = false;
    }
  }

  changeStatus(res, resLst, typeColumn) {
    if (!this.flagCursor) {
      if (res.header == 0) {
        if (typeColumn == 0) {
          this.putValueQuery(res)
        }
        else if (typeColumn == 1) {
          this.putValueNew(res)
        }
        else if (typeColumn == 2) {
          this.putValueIseditField(res)
        }
        else if (typeColumn == 3) {
          this.putValueIsdelete(res)
        }
      
        if (res.name == "") {
          for (var x = 0; x < resLst.lst.length; x++) {
            this.putAllValues(typeColumn, resLst.lst[x], res)
          }
        }
        else {
          this.removeAllValues(typeColumn, resLst.lst[1])
        }
      }
    }
  }

  putValueQuery(res) {
    if (res.isquery == 0) {
      res.isquery= 1;
    }
    else if (res.isquery == 1) {
      res.isquery = 2;
    }
    else {
      res.isquery = 0;
    }
  }

  putValueNew(res) {
    if (res.isnew == 0) {
      res.isnew= 1;
    }
    else if (res.isnew == 1) {
      res.isnew = 2;
    }
    else {
      res.isnew = 0;
    }
  }

  putValueIseditField(res) {
    if (res.iseditField == 0) {
      res.iseditField= 1;
    }
    else if (res.iseditField == 1) {
      res.iseditField = 2;
    }
    else {
      res.iseditField = 0;
    }
  }

  putValueIsdelete(res) {
    if (res.isdelete == 0) {
      res.isdelete= 1;
    }
    else if (res.isdelete == 1) {
      res.isdelete = 2;
    }
    else {
      res.isdelete = 0;
    }
  }

  putAllValues(typeColumn, reslst, res) {
    if (typeColumn == 0) {
      reslst.isquery = res.isquery;
    }
    else if (typeColumn == 1) {
      reslst.isnew = res.isnew;
    }
    else if (typeColumn == 2) {
      reslst.iseditField = res.iseditField;
    }
    else if (typeColumn == 3) {
      reslst.isdelete = res.isdelete;
    }
  }

  removeAllValues(typeColumn, reslst) {
    if (typeColumn == 0) {
      reslst.isquery = 0;
    }
    else if (typeColumn == 1) {
      reslst.isnew = 0;
    }
    else if (typeColumn == 2) {
      reslst.iseditField = 0;
    }
    else if (typeColumn == 3) {
      reslst.isdelete = 0;
    }
  }
}
