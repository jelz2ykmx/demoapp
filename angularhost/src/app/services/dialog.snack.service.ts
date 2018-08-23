import { Injectable } from '@angular/core';
import { MatSnackBar } from '@angular/material';

@Injectable({
  providedIn: 'root'
})
export class DialogSnackService {

  constructor(private snack: MatSnackBar) { }

  showSnack(message) {
    this.snack.open(message, "Cerrar", { duration: 3000 });
  }
}
