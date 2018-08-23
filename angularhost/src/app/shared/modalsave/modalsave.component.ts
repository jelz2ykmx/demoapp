import { Component, OnInit, Inject } from '@angular/core';
import { ModalspinnerComponent } from '../modalspinner/modalspinner.component';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';

@Component({
  selector: 'app-modalsave',
  templateUrl: './modalsave.component.html',
  styleUrls: ['./modalsave.component.scss']
})
export class ModalsaveComponent implements OnInit {

  constructor(
    public dialogRef: MatDialogRef<ModalspinnerComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) { }

  ngOnInit() {

  }

  onClick(value): void {
    this.dialogRef.close(value);
  }

}
