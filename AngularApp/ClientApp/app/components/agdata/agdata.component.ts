import { Component, Inject } from '@angular/core';
import { AuthService } from '../services/auth.service';

@Component({
    selector: 'agdata',
    templateUrl: './agdata.component.html'
})
export class AgDataComponent {
    public ags: any;
    public headers: any;

    constructor(authService: AuthService, @Inject('API_URL') apiUrl: string) {
        authService.get(apiUrl + 'AgContent/GetContents')
            .subscribe(result => {
                this.ags = result.json();
                this.headers = Object.keys(this.ags[0]);
                console.log(this.ags);
                console.log(this.headers);
            }, error => console.error(error));

    }

}
