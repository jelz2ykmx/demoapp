import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { FormBuilder, FormControl, Validators } from '@angular/forms';
import { DialogsDataService } from '../../../services/dialogs.data.service';
import { DomSanitizer } from '@angular/platform-browser';
import { DialogSnackService } from '../../../services/dialog.snack.service';
import { CharacterLimit, fuuidv4 } from '../../../helpers/text-helpers';
import { MenusRoles } from '../../../classes/menus.roles';

@Component({
  selector: 'app-companies-edit',
  templateUrl: './companies-edit.component.html',
  styleUrls: ['./companies-edit.component.scss']
})
export class CompaniesEditComponent implements OnInit {
  ngOnInit(): void {
    throw new Error("Method not implemented.");
  }
}
