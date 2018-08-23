import { Component, OnInit, Input, Output, EventEmitter, ViewChild, ElementRef } from '@angular/core';
import { FormBuilder, Validators, FormControl } from '@angular/forms';
import { CharacterLimit, fuuidv4, CharacterMinumun } from '../../../helpers/text-helpers';
import { DomSanitizer } from '@angular/platform-browser';
import { DialogsDataService } from '../../../services/dialogs.data.service';
import { MenusRoles } from '../../../classes/menus.roles';
import { DialogSnackService } from '../../../services/dialog.snack.service';

const URL = '';

@Component({
  selector: 'app-usuarios-edit',
  templateUrl: './usuarios-edit.component.html',
  styleUrls: ['./usuarios-edit.component.scss']
})
export class UsuariosEditComponent implements OnInit {
  
  newForm;
  title = "Nuevo";
  readonly = false;
  typeOperation = 0;
  menuLst = [];
  rolesLst = [];
  groupsLst = [];
  interval;
 
  isAdmin = false;
  isUserAdmin = false;
  imageRaw;
  file = "";
 
  notMaching = "Contraseñas no coinciden";
  minimunPasswordLength = 6;
  isShowBtnPhoto = true;
  menuRolesClass = new MenusRoles();
  
  @Input('id') id: string;
  @Input('path') path: string; 
  @Input('editQuery') editQuery: number;
  @Input('email') email: string;
  @Output() onSearch = new EventEmitter<any>();

  radios = 0;
  @ViewChild("labelPerfil", {read: ElementRef}) labelPerfil: ElementRef;
 
  goRadio(value) {
    this.radios = value;
  }
  
  constructor(private fb: FormBuilder, private dialogsService : DialogsDataService, public _DomSanitizer: DomSanitizer, private snack : DialogSnackService) { }

  ngOnInit() {
    if (this.id == "0") {
      this.newForm = this.fb.group ({
        email : new FormControl('', [ Validators.required, CharacterLimit(256), this.emailValid() ] ),
        firstname : new FormControl('', [ Validators.required, CharacterLimit(256)  ] ),
        lastname : new FormControl('', [ Validators.required, CharacterLimit(256)  ] ),
        direccion : new FormControl('', [ CharacterLimit(256)  ] ),
        colonia : new FormControl('', [ CharacterLimit(256)  ] ),
        ciudad : new FormControl('', [ CharacterLimit(256)  ] ),
        tel : new FormControl('', [ CharacterLimit(128)  ] ),
        password : new FormControl('', [ Validators.required, CharacterLimit(256), CharacterMinumun(this.minimunPasswordLength) ] ),
        retry : new FormControl('', [ Validators.required, CharacterLimit(256), CharacterMinumun(this.minimunPasswordLength) ] ),
        isAdmin : new FormControl('', ),
      },{ validator: this.matchingPassword()});

    }
    else {
      if (this.editQuery == 2) {
        this.newForm = this.fb.group ({
          email : new FormControl('', [ Validators.required, CharacterLimit(256), this.emailValid() ] ),
          password : new FormControl('', [ Validators.required, CharacterLimit(256), CharacterMinumun(this.minimunPasswordLength) ] ),
          retry : new FormControl('', [ Validators.required, CharacterLimit(256), CharacterMinumun(this.minimunPasswordLength) ] ),
        },{ validator: this.matchingPassword()});
        this.newForm.controls['email'].setValue(this.email);
        this.newForm.controls['email'].disable();
        this.typeOperation = 3;
        this.title = "Cambiar contraseña"
      }
      else {
        this.newForm = this.fb.group ({
          email : new FormControl('', [ Validators.required, CharacterLimit(256), this.emailValid() ] ),
          firstname : new FormControl('', [ Validators.required, CharacterLimit(256)  ] ),
          lastname : new FormControl('', [ Validators.required, CharacterLimit(256)  ] ),
          direccion : new FormControl('', [ CharacterLimit(256)  ] ),
          colonia : new FormControl('', [ CharacterLimit(256)  ] ),
          ciudad : new FormControl('', [ CharacterLimit(256)  ] ),
          tel : new FormControl('', [ CharacterLimit(128)  ] ),
          isAdmin : new FormControl('', ),
        });
        if (this.editQuery == 1) {
          this.title = "Editar";
          this.typeOperation = 1;
        }
        else {
          this.title = "Consulta";
          this.typeOperation = 2;
          this.isShowBtnPhoto = false;
        }

        
      }
    }
    if (this.id == "0" || this.editQuery != 2) {
      this.newForm.get('email')['tagname'] = 'correo';
      this.newForm.get('firstname')['tagname'] = 'nombre';
      this.newForm.get('lastname')['tagname'] = 'apellido';
      this.newForm.get('tel')['tagname'] = 'telefono';
    }
    if (this.id == "0" || this.editQuery == 2) {
      this.newForm.get('password')['tagname'] = "contraseña, minimo " +  this.minimunPasswordLength + " caracteres";
      this.newForm.get('retry')['tagname'] = "verifique contraseña, minimo " +  this.minimunPasswordLength + " caracteres";
    }
    
    this.interval = setInterval( () => { 
      clearInterval(this.interval);
      this.getById();
    });
  }

  ngAfterViewInit(): void {
    if (this.id == "0" || this.editQuery != 2) {
      this.labelPerfil.nativeElement.click();
    }
  }

  isValid(control) {
    return this.newForm.controls[control].invalid && this.newForm.controls[control].touched;
  }

  getById() {
    var model = {
        type : 2,
        byId : {
          id : this.id,
        }
    }
        
    this.dialogsService.runWebservices(this.path, model, 1)
    .then( data => {
      if (data != null ) {
        if (data.email != undefined) {
          
          if (this.id == "0" || this.editQuery != 2) {
            this.newForm.controls['email'].setValue(data.email);
            this.newForm.controls['firstname'].setValue(data.firstname);
            this.newForm.controls['lastname'].setValue(data.lastname);
            this.newForm.controls['direccion'].setValue(data.direccion);
            this.newForm.controls['colonia'].setValue(data.colonia);
            this.newForm.controls['ciudad'].setValue(data.ciudad);
            this.newForm.controls['tel'].setValue(data.tel);
            this.file = "data:image/jpeg;base64," + data.photo;
          }
          
          if (this.id != "0" && this.editQuery == 0) {
            this.newForm.controls['email'].disable();
            this.newForm.controls['firstname'].disable();
            this.newForm.controls['lastname'].disable();
            this.newForm.controls['direccion'].disable();
            this.newForm.controls['colonia'].disable();
            this.newForm.controls['ciudad'].disable();
            this.newForm.controls['tel'].disable();
            this.newForm.controls['isAdmin'].disable();
            this.readonly = true;
          }
          else { 
            this.readonly = false;
          }
          this.newForm.controls['isAdmin'].setValue(false);
          if (data.isrootadmin)  { 
            this.isAdmin = true;
            if (data.isadmin) { 
              this.newForm.controls['isAdmin'].setValue(true);
              this.isUserAdmin = true;
            }
          }
          else { 
            this.newForm.controls['isAdmin'].disable();
          }

          this.menuRolesClass.extractData(data.menu, this.menuLst, 0);
          this.menuRolesClass.extractData(data.roles, this.rolesLst, 1);
          if (this.id != "0") {
            this.menuRolesClass.reorderModel(this.menuLst);
          }
          this.groupsLst = this.menuRolesClass.addOriginalValue(data.groups);
        }
        else {
          this.readonly = true;
        }
      }
    });
  }
 
  matchingPassword() {
    return form => {
     if (form.controls["password"].value != form.controls["retry"].value) {
        return { notMatchingPassword : true}
      }
    }
  }

  emailValid() {
    return control => {
      var regex = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/
      return regex.test(control.value) ? null : { invalidEmail : true};
    }
  }

  showConfirmacion() {
    if (!this.dialogsService.checkError(this.newForm)) {
      if (!this.newForm.valid) {
        this.moveButtonPosition();
        this.snack.showSnack(this.notMaching);
      }
      else {
        let dialogRef = this.dialogsService.createView(0);
        dialogRef.afterClosed().subscribe(result => {
          if (result != 0) {
            if (result == 1) {
               this.doUpdate();
            }
          }
          else {
            this.doConsulta(undefined);
          }
        });
      }
    }
    else {
      this.moveButtonPosition();
    }
  }
  
  moveButtonPosition() {
    this.radios = 0;
    this.labelPerfil.nativeElement.click();
  }

  doUpdate() {
    var model;
    if (this.editQuery == 2) {
      model = {
        type : 6,
        changePassword : {
          id : this.id,
          password : this.newForm.get("password").value 
        }
      }
    }
    else if (this.id == '0') {
      model = this.CreateUpdateModel(fuuidv4(), 3);
    }
    else {
      model = this.CreateUpdateModel(this.id, 4);
    }
    this.runWebservices(model);
  }

  CreateUpdateModel(id, type) {
    var model = {
      type : type,
      update : {
        id : id,
        email : this.newForm.get("email").value,
        firstname : this.newForm.get("firstname").value,
        lastname : this.newForm.get("lastname").value,
        direccion : this.newForm.get("direccion").value,
        colonia : this.newForm.get("colonia").value,
        ciudad : this.newForm.get("ciudad").value,
        tel : this.newForm.get("tel").value,
        photo : this.file.replace(/data:image\/jpeg;base64,/g, ''),
        menu : [],
        roles : [],
        groups : [],
      }
    };
    if (this.id == "0") {
      model.update["password"] = this.newForm.get("password").value;
    }
    if (!this.isUserAdmin) {
      model.update["isUserAdmin"] = 0;
      this.menuRolesClass.pushDataModel(this.menuLst, model.update);
      this.menuRolesClass.pushValuesModel(model.update.groups, this.groupsLst, 1);
    }
    else {
      model.update["isUserAdmin"] = 1;
    }
    return model;
  }

  runWebservices(model) {
    this.dialogsService.runWebservices(this.path, model, 0)
    .then( data => {
      if (data != null && data.error != undefined) {
      }
      else {
        var idvalue = 0;
        if (this.editQuery == 2) {
          idvalue = model.changePassword.id;
        }
        else if (this.id == "0") {
          idvalue = data.id;
        }
        else {
          idvalue = model.update.id;
        }
        this.doConsulta(idvalue);
      }
    });
  }
  
  doConsulta(id) {
    var model = {
      typeOperation : this.typeOperation
    }
    if (this.typeOperation != 3) 
    {
      if (id == undefined) {
        this.typeOperation = -1;
      }
      var name = this.newForm.get('firstname').value + " " + this.newForm.get('lastname').value;
      if (this.newForm.get('firstname').value == "" && this.newForm.get('firstname').value == "")  {
        name = this.newForm.get('email').value;
      }
      model["id"] = id;
      model["email"] = this.newForm.get('email').value;
      model["name"] = name;
      model["typeOperation"] = this.typeOperation;
    }
    
    this.onSearch.emit(model);
  }

  onFileSelected(e) {
    var reader = new FileReader();
    reader.onload = (event:any) => {
     this.file = event.target.result;
    }
    reader.readAsDataURL(e.srcElement.files[0]);
  }
 
  doToogle() {
    this.isUserAdmin = !this.isUserAdmin; 
  }

}
