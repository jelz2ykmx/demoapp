import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Routes, RouterModule } from '@angular/router';
import { LayoutmanagerAppComponent } from './layoutmanager-app.component';
import { ToolbarComponent } from './components/toolbar/toolbar.component';
import { MainContentComponent } from './components/main-content/main-content.component';
import { SidenavComponent } from './components/sidenav/sidenav.component';
import { MaterialModule } from '../shared/material.module';
import { FlexLayoutModule } from '@angular/flex-layout';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { BreadcumComponent } from './components/breadcum/breadcum.component';
import { BreadcumDirective } from './components/sidenav/breadcum.directive';



const routes: Routes = [
  { path: '', component: LayoutmanagerAppComponent,
    children: [
      { path: '', component: MainContentComponent },
      { 
        path: 'Configuracion', 
        loadChildren: '../configuracion/configuracion.module#ConfiguracionModule' 
      }
    ]
  }
];

@NgModule({
  imports: [
    CommonModule,
    MaterialModule,
    FlexLayoutModule,
    HttpClientModule,
    FormsModule,
    RouterModule.forChild(routes)
  ],
  declarations: [LayoutmanagerAppComponent, ToolbarComponent, MainContentComponent, SidenavComponent, BreadcumComponent,
  BreadcumDirective],
  entryComponents: [BreadcumComponent]
})
export class LayoutmanagerModule { }
