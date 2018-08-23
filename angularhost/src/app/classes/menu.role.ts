export class MenuRole {

    validFormat() {
        return control => {
            var value = control.value.toString().trim();
            var index = value.indexOf(' ');
            if (index == -1) {
               index = value.indexOf('/');
            }
            else {
                index = -1;
            }
            return  (index == -1 || index == (value.length -1) ) ?  { field  : true } : null
        }
    }
}
