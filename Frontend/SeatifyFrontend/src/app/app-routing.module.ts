import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LandingPageComponent } from './pages/landing-page/landing-page.component';
import { VenueDashboardComponent } from './pages/venue-dashboard/venue-dashboard.component';

export const routes: Routes = [
  { path: '', component: LandingPageComponent },
  { path: 'landingpage', component: LandingPageComponent },
  { path: 'venues', component: VenueDashboardComponent },
  { path: '**', redirectTo: '' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
