import { Injectable } from '@angular/core';
import { SwUpdate } from '@angular/service-worker';
import { MatSnackBar } from '@angular/material';

@Injectable()
export class UpdateService {

  constructor(private swUpdate: SwUpdate, private snackbar: MatSnackBar) {
      if (!swUpdate.isEnabled) {
        console.log('Nope 🙁' );
      }  
      swUpdate.available.subscribe(evt => {
        const snack = snackbar.open('Actualizacion disponible', 'Actualizar', { duration: 6000 });
        snack.onAction().subscribe(() => {
            window.location.reload();
        });
    }); 
    
  }

  checkForUpdate() {
    this.swUpdate.checkForUpdate()
      .then(() => {
        const snack = this.snackbar.open('Actualizacion disponible', 'Actualizar');
        snack.onAction().subscribe(() => {
            window.location.reload();
        });
      })
      .catch(err => {
        console.error(err);
      })
  }

  activateUpdate() {
    console.log('[App] activateUpdate started')
    this.swUpdate.activateUpdate()
      .then(() => {
        console.log('[App] activateUpdate completed')
        window.location.reload()
      })
      .catch(err => {
        console.error(err);
      })
  }

}
