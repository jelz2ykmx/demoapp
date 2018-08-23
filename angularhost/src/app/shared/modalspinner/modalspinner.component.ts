import { Component, OnInit, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';

@Component({
  selector: 'app-modalspinner',
  templateUrl: './modalspinner.component.html',
  styleUrls: ['./modalspinner.component.scss']
})
export class ModalspinnerComponent implements OnInit {

  constructor(
    public dialogRef: MatDialogRef<ModalspinnerComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) { }

  ngOnInit() {

  }

  onNoClick(): void {
    this.dialogRef.close();
  }
}
