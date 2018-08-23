import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NgModule } from '@angular/core';

import { AppComponent } from './app.component';
import { Routes, RouterModule } from '@angular/router';
import { AcessguardService } from './services/acessguard.service';
import { AuthService } from './services/auth.service';
import { WebservicesService } from './services/webservices.service';
import { HttpClientModule } from '@angular/common/http';
import { MatSnackBarModule, MatDialogModule } from '@angular/material';
import { ServiceWorkerModule } from '@angular/service-worker';
import { environment } from '../environments/environment';
import { UpdateService } from './services/update.service';
import { ConfigService } from './services/config.service';
import { MenuItemsService } from './services/menu.items.service';

const routes: Routes = [
  {
    path: '',
    loadChildren: './layoutmanager/layoutmanager.module#LayoutmanagerModule' ,
    data: { requiresLogin: true },
    canActivate: [ AcessguardService ]
  },
  { 
    path: 'login', 
    loadChildren: './usertasks/usertasks.module#UsertasksModule' 
  },
  { 
    path: 'notfound', 
    loadChildren: './info/info.module#InfoModule' 
  },
  { path: '**', 
    redirectTo: 'notfound' 
  }
];

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    HttpClientModule,
    MatSnackBarModule,
    MatDialogModule,
    ServiceWorkerModule.register('/ngsw-worker.js', {
      enabled: environment.production
    }),
    RouterModule.forRoot(routes)
  ],
  providers: [ AcessguardService, AuthService, WebservicesService, UpdateService, ConfigService, MenuItemsService ],
  bootstrap: [AppComponent]
})
export class AppModule { }
