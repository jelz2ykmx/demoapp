import { Directive, ViewContainerRef } from '@angular/core';

@Directive({
  selector: '[app-breadcum]',
})
export class BreadcumDirective {
  constructor(public viewContainerRef: ViewContainerRef) { }
}