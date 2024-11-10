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

export const routes: Routes = [
    {path: '', component: HomeComponent},
    {path: 'register', component: RegisterComponent},
    {path: 'login', component: LoginComponent},
    {path: 'profile', component: ProfileComponent },
    {path: 'edit', component: EditComponent },
    {path: 'users', component: UsersComponent },
    {path: 'auth/emailconfirmation', component: EmailConfirmationComponent },
    {path: 'auth/twofactor', component: TwofactorComponent }
];

@NgModule({
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule]
  })
  export class AppRoutingModule { }