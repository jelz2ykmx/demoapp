import { Injectable } from '@angular/core';

@Injectable()
export class ConfigService {
    BASE_URL = 'http://localhost:30270/';
    BASE_SIGNALR = this.BASE_URL + "notify";
}
