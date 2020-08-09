import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { RouterModule } from '@angular/router';
import { HttpClientModule } from '@angular/common/http';

import { AppComponent } from './components/app/app.component'
import { NavMenuComponent } from './components/navmenu/navmenu.component';
import { HomeComponent } from './components/home/home.component';
import { FetchDataComponent } from './components/fetchdata/fetchdata.component';

import { AgDataComponent } from './components/agdata/agdata.component';

import { CounterComponent } from './components/counter/counter.component';
import { UnauthorizedComponent } from './components/unauthorized/unauthorized.component';

import { AuthModule, OidcSecurityService } from 'angular-auth-oidc-client';
import { AuthService } from './components/services/auth.service';

@NgModule({
    declarations: [
        AppComponent,
        NavMenuComponent,
        CounterComponent,
        FetchDataComponent,
        AgDataComponent,
        HomeComponent,
        UnauthorizedComponent
    ],
    imports: [
        AuthModule.forRoot(),
        CommonModule,
        HttpModule,
        HttpClientModule,
        FormsModule,
        RouterModule.forRoot([
            { path: '', redirectTo: 'home', pathMatch: 'full' },
            { path: 'home', component: HomeComponent },
            { path: 'unauthorized', component: UnauthorizedComponent },
            { path: 'counter', component: CounterComponent },
            { path: 'fetch-data', component: FetchDataComponent },
            { path: 'ag-data', component: AgDataComponent },
            { path: '**', redirectTo: 'home' }
        ])
    ],
    providers: [
        AuthService,
        OidcSecurityService
    ]
})
export class AppModuleShared {
}

