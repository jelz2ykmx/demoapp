import { Component, OnInit, NgZone, ViewChild, ComponentFactoryResolver } from '@angular/core';
import { Router } from '@angular/router';
import { MatSidenav, MatCard, MatDialog } from '@angular/material';
import { AuthService } from '../../../services/auth.service';
import { BreadcumDirective } from './breadcum.directive';
import { BreadcumComponent } from '../breadcum/breadcum.component';
import { WebservicesService } from '../../../services/webservices.service';
import { ModalspinnerComponent } from '../../../shared/modalspinner/modalspinner.component';
import { MenuItemsService } from '../../../services/menu.items.service';

const SMALL_WIDTH_BREAKPOINT = 720;

@Component({
  selector: 'app-sidenav',
  templateUrl: './sidenav.component.html',
  styleUrls: ['./sidenav.component.scss']
})
export class SidenavComponent implements OnInit {
  @ViewChild(MatSidenav) sidenav: MatSidenav;
  @ViewChild(BreadcumDirective) breadcumHost: BreadcumDirective;
  
  breadcum = "";
  
  private mediaMatcher: MediaQueryList = matchMedia(`(max-width: ${SMALL_WIDTH_BREAKPOINT}px)`);

  menuItems = [];
  /*
  menuItems = [
    { 
      'avatar' : 'Catalogos',
      'menuItem' : 'Configuracion',
      'subMenuitems' : [
        {
          'avatar' : 'Catalogos',
          'menuItem' : 'Menu',
          'route' : 'Configuracion/Menu'
        },
        {
          'avatar' : 'Catalogos',
          'menuItem' : 'Grupos',
          'route' : 'Configuracion/Grupos'
        },
        {
          'avatar' : 'Catalogos',
          'menuItem' : 'Usuarios',
          'route' : 'Configuracion/Usuarios'
        }
      ]
    },
    { 
      'avatar' : 'Catalogos',
      'menuItem' : 'Transacciones',
      'subMenuitems' : [
      ]
    },
    { 
      'avatar' : 'Catalogos',
      'menuItem' : 'Reportes',
      'subMenuitems' : [
      ]
    }
  ];
*/
  constructor(zone: NgZone, private router: Router, public auth: AuthService, private componentFactoryResolver: ComponentFactoryResolver,
    private webservices: WebservicesService, public dialog: MatDialog, private menurights : MenuItemsService) { 

    this.mediaMatcher.addListener(mql => 
      zone.run(() => this.mediaMatcher = mql));

    let dialogRef = this.dialog.open(ModalspinnerComponent,  {
      width: '250px',
      disableClose: true,
      panelClass: 'spinner-dialog'
      //data: { name: this.name, animal: this.animal }
    });
    
    var model = 
    {
      type : 7
    };
    this.webservices.postMessage('api/menurightstoken', model)
    .then( data => {
        if (data != null) {
          this.createMenu(data);  
        }
        dialogRef.close();
    }).catch( err => {
      dialogRef.close();
    });
  
  }

  ngOnInit() {
    
    this.router.events.subscribe(() => {
      if (this.isScreenSmall()) {
        this.sidenav.close();
      }
    });
    this.breadcum = "DashBoard";
    //this.loadComponent(true, "");
  }

  createMenu(data) {
    var count = 0;
    for ( var x = 0; x < data.length; x++) {
      var index = data[x].menu.indexOf('/');
      var itemParent = data[x].menu.substring(0,index);
      var modelParent = {
        'avatar' : 'Catalogos',
        'menuItem' : itemParent,
        'subMenuitems' : []
      }
      this.menuItems[count] = modelParent;
      for ( var y = x; y < data.length; y++) {
        var itemParentTemp = data[y].menu.substring(0,index);
        if (itemParentTemp != itemParent) {
          break;
        }
        var itemChildren = data[y].menu.substring( index + 1);
        var modelrights =  {
            isquery : data[y].isquery,
            isedit :  data[y].isedit,
            isnew : data[y].isnew,
            isdelete : data[y].isdelete
        }
        this.menurights.menuItemsRights[data[y].menu] = modelrights;
        var modelChildren = {
          'avatar' : 'Catalogos',
          'menuItem' : itemChildren,
          'route' : data[y].menu
        }
        this.menuItems[count].subMenuitems.push(modelChildren);
        x = y
      }
      count++;
    }
  }

  isScreenSmall() : boolean {
    return this.mediaMatcher.matches;
  }

  doLink(menuItem) {
    this.breadcum = menuItem;
   
    //this.loadComponent(true, path)
  }

  loadComponent(isAdding, path) {
    let componentFactory = this.componentFactoryResolver.resolveComponentFactory(BreadcumComponent);

    let viewContainerRef = this.breadcumHost.viewContainerRef;
    viewContainerRef.clear();
    
    if (isAdding) {
      let componentRef = viewContainerRef.createComponent(componentFactory);
      (<BreadcumComponent>componentRef.instance).breadcum = path;
    }
  }
}
