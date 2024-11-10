import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { NgModule } from '@angular/core';
import { RegisterComponent } from './register/register.component';
import { LoginComponent } from './login/login.component';
import { ProfileComponent } from './profile/profile.component';
import { EditComponent } from './edit/edit.component';
import { UsersComponent } from './users/users.component';
import { EmailConfirmationComponent } from './email-confirmation/email-confirmation.component';
import { TwofactorComponent } from './twofactor/twofactor.component';
import { authGuard } from './_guards/auth.guard';
import { adminGuard } from './_guards/admin.guard';
import { nonauthGuard } from './_guards/nonauth.guard';
import { NotFoundComponent } from './errors/not-found/not-found.component';
import { ServerErrorComponent } from './errors/server-error/server-error.component';

export const routes: Routes = [
    {path: '', component: HomeComponent, canActivate: [nonauthGuard]},
    {path: 'register', component: RegisterComponent, canActivate: [nonauthGuard]},
    {path: 'login', component: LoginComponent, canActivate: [nonauthGuard]},
    {path: 'profile', component: ProfileComponent, canActivate: [authGuard] },
    {path: 'edit', component: EditComponent, canActivate: [authGuard] },
    {path: 'users', component: UsersComponent, canActivate: [adminGuard] },
    {path: 'auth/emailconfirmation', component: EmailConfirmationComponent },
    {path: 'auth/twofactor', component: TwofactorComponent, canActivate: [nonauthGuard]  },
    {path: 'not-found', component: NotFoundComponent },
    {path: 'server-error', component: ServerErrorComponent },
    { path: '**', component: NotFoundComponent }
];

@NgModule({
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule]
  })
  export class AppRoutingModule { }