export class MenusRoles {
    
    extractData(data, lstType, typeModel) {
        if (data != undefined) {
            var count = 0;
            var model = this.createExtractModel("", count);
            if (data.length > 0) {
                var names = data[0].name.split("/");
                if (names.length > 1) {
                    model.name = names[0]
                    if (typeModel == 0) {
                        this.pushExtractModel(model,this.createHeaderModel(typeModel, 0, 1), "", typeModel);
                    }
                    this.pushExtractModel(model,this.createHeaderModel(typeModel, 1, 0), "", typeModel);
                }
                for (var x = 0; x < data.length; x++) {
                    var temp = data[x].name.split("/");
                    if (x == (data.length -1) || temp[0] != names[0]) {
                        names = temp;
                        lstType.push(model);
                        count++;
                        model = this.createExtractModel( names[0], count);
                        if (typeModel == 0) {
                            this.pushExtractModel(model,this.createHeaderModel(typeModel, 0, 1), "", typeModel);
                        }
                        this.pushExtractModel(model, this.createHeaderModel(typeModel, 1, 0), "", typeModel);
                        this.pushExtractModel(model, data[x], temp[1], typeModel);
                        }
                    else {
                        if (temp.length > 1) {
                            this.pushExtractModel(model, data[x], temp[1], typeModel);
                        }
                    }
                } 
                if (model.name != "") {
                    if (typeModel == 1) {
                        lstType[count-1].lst.push(model.lst[1]);
                    }
                    else {
                        lstType[count-1].lst.push(model.lst[2]);
                    }
                }
            }
        }
    }
    
    createExtractModel(name, group) {
        var model = 
        {
            name : name,
            group : group,
            lst : []
        };
        return model;
    }
    
    createHeaderModel(typeModel, isEdit, header) {
        var temp = {
            id : 0,
            name : "",
            isEdit : isEdit
        };
        if (typeModel == 0) {
            temp['header'] = header;
            temp['isquery'] = 0;
            temp['isqueryOriginal'] = 0;
            temp['isnew'] = 0;
            temp['isnewdOriginal'] = 0;
            temp['iseditField'] = 0;
            temp['iseditFielddOriginal'] = 0;
            temp['isdelete'] = 0;
            temp['isdeleteOriginal'] = 0;
        }
        else {
            temp['typeRight'] = 0;
            temp['typeOriginal'] = 0;
        }
        return  temp;
    }
    
    pushExtractModel(model, data, name, typeModel) {
        var temp = {
            id : data.id,
            name : name,
            isEdit : data.isEdit
        };
        if (typeModel == 0) {
            if (data.header == undefined) {
                temp['header'] = 0;
            }
            else {
                temp['header'] = data.header;
            }
            temp['isquery'] = data.isquery;
            temp['isqueryOriginal'] = data.isquery;
            temp['isnew'] = data.isnew;
            temp['isnewdOriginal'] = data.isnew;
            temp['iseditField'] = data.iseditField;
            temp['iseditFielddOriginal'] = data.iseditField;
            temp['isdelete'] = data.isdelete;
            temp['isdeleteOriginal'] = data.isdelete;
        }
        else {
            temp['typeRight'] = data.typeRight;
            temp['typeOriginal'] = data.typeRight;
        }
        model.lst.push(temp);
    }
    
    reorderModel(menuLst) {
        for (var x = 0; x < menuLst.length; x++) {
            var typer = menuLst[x].lst[2].isquery;
            var typen = menuLst[x].lst[2].isnew;
            var typee = menuLst[x].lst[2].iseditField;
            var typed = menuLst[x].lst[2].isdelete;
            for (var y = 3; y < menuLst[x].lst.length; y++) {
                var flagr = false;
                var flagn = false;
                var flage = false;
                var flagd = false;
                if (menuLst[x].lst[y].isquery != typer) {
                    typer = 0;
                    flagr = true;
                }
                if (menuLst[x].lst[y].isnew != typen) {
                    typen = 0;
                    flagn = true;
                }
                if (menuLst[x].lst[y].iseditField != typee) {
                    typee = 0;
                    flage = true;
                }
                if (menuLst[x].lst[y].isdelete != typed) {
                    typed = 0;
                    flagd = true;
                }
                if (flagr && flagn && flage && flagd) {
                    break;
                }
            }
            menuLst[x].lst[1].isquery = typer;
            menuLst[x].lst[1].isnew = typen;
            menuLst[x].lst[1].iseditField = typee;
            menuLst[x].lst[1].isdelete = typed;
        }
        
    }

    pushDataModel(menuLst, model) 
    {
        for (var x = 0; x < menuLst.length; x++) {
            for (var y = 0; y < menuLst[x].lst.length; y++) {
                if (menuLst[x].lst[y].id != 0) {
                    if (menuLst[x].lst[y].isquery != menuLst[x].lst[y].isqueryOriginal ||
                        menuLst[x].lst[y].isnew != menuLst[x].lst[y].isnewdOriginal ||
                        menuLst[x].lst[y].iseditField != menuLst[x].lst[y].iseditFielddOriginal ||
                        menuLst[x].lst[y].isdelete != menuLst[x].lst[y].isdeleteOriginal) {
                        model.menu.push( 
                            {
                                idmenu : menuLst[x].lst[y].id,
                                isquery : menuLst[x].lst[y].isquery,
                                isnew : menuLst[x].lst[y].isnew,
                                iseditField : menuLst[x].lst[y].iseditField,
                                isdelete : menuLst[x].lst[y].isdelete,
                                isedit : menuLst[x].lst[y].isEdit
                            }
                        );
                    }
                }
            }
        }
    }

    addOriginalValue(data) {
        for (var x = 0; x < data.length; x++ ) {
          data[x]['ischeckedOriginal'] = data[x].ischecked;
        }
        return data;
    }

    pushValuesModel(model, lst, type) {
        for (var x = 0; x < lst.length; x++) {
          if (lst[x].ischecked != lst[x].ischeckedOriginal) {
            var modelTmp = {};
            if (type == 0) 
            {
                modelTmp['iduser'] = lst[x].id;
            }
            else
            {
                modelTmp['idgroup'] = lst[x].id;
            }
            modelTmp['ischecked'] = lst[x].ischecked;
            modelTmp['isedit'] = lst[x].isEdit;
            model.push(modelTmp);
          }
        }
      }
}
