import { Component, OnInit } from '@angular/core';
import { MatIconRegistry } from '@angular/material';
import { DomSanitizer } from '@angular/platform-browser';

@Component({
  selector: 'app-layoutmanager-app',
  template: `
    <app-sidenav></app-sidenav>
  `,
  styles: []
})
export class LayoutmanagerAppComponent implements OnInit {

  constructor(iconRegistry: MatIconRegistry, sanitizer: DomSanitizer) {
    iconRegistry.addSvgIcon('Catalogos', (sanitizer.bypassSecurityTrustResourceUrl('assets/icons/avatars.svg')));
  }

  ngOnInit() {
  }

}
