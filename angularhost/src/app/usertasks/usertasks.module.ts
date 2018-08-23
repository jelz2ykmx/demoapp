import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { UsertasksRoutingModule } from './usertasks-routing.module';
import { LoginComponent } from './components/login/login.component';
import { MaterialModule } from '../shared/material.module';
import { FlexLayoutModule } from '@angular/flex-layout';
import { FormsModule } from '@angular/forms';


@NgModule({
  imports: [
    CommonModule,
    MaterialModule,
    FlexLayoutModule,
    FormsModule,
    UsertasksRoutingModule
  ],
  declarations: [LoginComponent]
})
export class UsertasksModule { }
