import { Component, ViewChild } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { DialogsDataService } from '../../../services/dialogs.data.service';
import * as jsPDF from 'jspdf'
import { MenuItemsService } from '../../../services/menu.items.service';

@Component({
  selector: 'app-groups',
  templateUrl: './groups.component.html',
  styleUrls: ['./groups.component.scss']
})
export class GroupsComponent {
  routingPath = 'Configuracion/Grupos';
  path = "api/groups";
  basicForm;

  title = "Búsqueda";

  filter = "";
  status = 1;
  resultLst = [];
 
  search = true;
  edit = false;
  viewFilter = false;
  isnew = false;

  id = '0';
  editQuery = 0;
 
  constructor(private fb: FormBuilder, private dialogsService : DialogsDataService, private menurights : MenuItemsService) { 
    this.basicForm = this.fb.group ({
      filter: ["",[] ],
    });
    if (menurights.menuItemsRights[this.routingPath].isnew == 1) {
      this.isnew = true;
    }
  }

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
      if (data.error == undefined) {
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
        name : model.name,
        status : 1
      }  
      this.resultLst.splice(0, 0, tmpmodel);
    }   
    else if (model.typeOperation == 1) {
      for (var x = 0; x < this.resultLst.length; x++) {
        var tmp = this.resultLst[x] as any;
        if (tmp.id == model.id) {
            this.resultLst[x].name = model.name;
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

  doReport() {
    let doc = new jsPDF();
    doc.text(20,20,'Hello world');
    var img = new Image;
    img.onload = function() {
      doc.addImage(this, 30, 30, 5, 5);
      doc.save("test.pdf");
    };
    img.crossOrigin = "";  // for demo as we are at different origin than image
    img.src = "assets/icons/avatars.svg";  // some random imgur image
    
    //doc.save('Test.pdf');
  }

    
  template = '<html xmlns:o="urn:schemas-microsoft-com:office:office" xmlns:x="urn:schemas-microsoft-com:office:excel" xmlns="http://www.w3.org/TR/REC-html40"><head><!--[if gte mso 9]><xml><x:ExcelWorkbook><x:ExcelWorksheets><x:ExcelWorksheet><x:Name>{worksheet}</x:Name><x:WorksheetOptions><x:DisplayGridlines/></x:WorksheetOptions></x:ExcelWorksheet></x:ExcelWorksheets></x:ExcelWorkbook></xml><![endif]--></head><body><table>{table}</table></body></html>';
  uri = 'data:application/vnd.ms-excel;base64,';
  interval;

  doExcel() {
    var exportHref = this.tableToExcel('WireWorkbenchDataExport');
    this.interval = setInterval( () => { 
      clearInterval(this.interval);
      location.href = exportHref;
     },100);
  }

  tableToExcel(worksheetName) {
    
    var table =  
      '<table border="1">' +
        '<thead>' +
          '<tr class="table-header">' +
            '<th>Team</th>' +
            '<th>Process Type</th>' +
            '<th>Cedent</th>' +
          '</tr>' +
        '</thead>' +
        '<tbody>' +
          '<tr>' +
            '<td>value 1</td>' +
            '<td>value 2</td>' +
            '<td>value 3</td>' +
          '</tr>' +
          '<tr>' +
            '<td>value 4</td>' +
            '<td>value 5</td>' +
            '<td>value 6</td>' +
          '</tr>' +
          '<tr>' +
            '<td>10.12.2015</td>' +
            '<td>AXA Affin</td>' +
            '<td>101024 - Quota Share QS</td>' +
          '</tr>' +
        '</tbody>' +
      '</table>';
   
    var elem = document.createElement("div"); 
    elem.innerHTML = table;
    var ctx = {
      worksheet : worksheetName, 
      table : elem.innerHTML
    }

    var href = this.uri + this.base64(this.format(this.template,ctx));
    return href;
  }

  base64(s) {
    return btoa(s);
  }

  format(s,c) {
    return s.replace(/{(\w+)}/g,function(m,p){return c[p];})
  };
    
}
