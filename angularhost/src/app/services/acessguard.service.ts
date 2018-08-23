import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot } from '@angular/router';
import { AuthService } from './auth.service';
import { Observable } from 'rxjs';

@Injectable()
export class AcessguardService implements CanActivate {
  
  constructor(private auth : AuthService) {}

  canActivate(route: ActivatedRouteSnapshot): Observable<boolean>|Promise<boolean>|boolean {
    
    const requiresLogin = route.data['requiresLogin'] || false;
    if (requiresLogin) {
       let flag = this.auth.isAuthenticated;
       if (!flag) {
          this.auth.logout();  
       }
       return flag; 
    }

    return true;
  }
 
}