import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ConfiguracionRoutingModule } from './configuracion-routing.module';
import { MaterialModule } from '../shared/material.module';
import { FlexLayoutModule } from '@angular/flex-layout';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { GroupsComponent } from './groups/groups/groups.component';
import { MenuComponent } from './menu/menu/menu.component';
import { MenuEditComponent } from './menu/menu-edit/menu-edit.component';
import { MenuFiltroComponent } from './menu/menu-filtro/menu-filtro.component';
import { MenuResultComponent } from './menu/menu-result/menu-result.component';
import { GroupsEditComponent } from './groups/groups-edit/groups-edit.component';
import { GroupsFiltroComponent } from './groups/groups-filtro/groups-filtro.component';
import { GroupsResultComponent } from './groups/groups-result/groups-result.component';
import { GroupsMenuComponent } from './groups/groups-menu/groups-menu.component';
import { UsuariosComponent } from './usuarios/usuarios/usuarios.component';
import { UsuariosResultComponent } from './usuarios/usuarios-result/usuarios-result.component';
import { UsuariosFiltroComponent } from './usuarios/usuarios-filtro/usuarios-filtro.component';
import { UsuariosEditComponent } from './usuarios/usuarios-edit/usuarios-edit.component';
import { DialogsDataService } from '../services/dialogs.data.service';
import { UsuariosGruposComponent } from './usuarios/usuarios-grupos/usuarios-grupos.component';
import { CompaniesComponent } from './companies/companies/companies.component';
import { CompaniesEditComponent } from './companies/companies-edit/companies-edit.component';
import { CompaniesFiltroComponent } from './companies/companies-filtro/companies-filtro.component';
import { CompaniesUsersComponent } from './companies/companies-users/companies-users.component';
import { CompaniesResultComponent } from './companies/companies-result/companies-result.component';



@NgModule({
  imports: [
    CommonModule,
    MaterialModule,
    FlexLayoutModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    ConfiguracionRoutingModule
  ],
  declarations: [ GroupsComponent, MenuComponent, 
    MenuEditComponent, MenuFiltroComponent, MenuResultComponent, GroupsEditComponent, GroupsFiltroComponent, GroupsResultComponent, 
    GroupsMenuComponent, UsuariosComponent, UsuariosResultComponent, UsuariosFiltroComponent, UsuariosEditComponent, UsuariosGruposComponent, CompaniesComponent, CompaniesEditComponent, CompaniesFiltroComponent, CompaniesUsersComponent, CompaniesResultComponent],
  providers: [ DialogsDataService ],
})
export class ConfiguracionModule { }
