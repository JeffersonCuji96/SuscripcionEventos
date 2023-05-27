import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { environment } from 'src/environments/environment';
import { AuthService } from '../modules/auth/services/auth.service';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class HubService {

  readonly urlApi = environment.urlHost;
  private connectionHub: signalR.HubConnection;

  constructor(private authService: AuthService,private http:HttpClient) {
    this.connectionHub = new signalR.HubConnectionBuilder().withUrl(
      this.urlApi + "suscription-hub", {
      accessTokenFactory: () => this.authService.getToken()
    }).configureLogging(signalR.LogLevel.Warning).build();
  }
  
  getConnectionHub():signalR.HubConnection {
    return this.connectionHub;
  }

  getSuscriptionsBeforeJoinGroup(id:number){
    return this.http.get<any>(this.urlApi + "api/Suscripcion/GetSuscriptionsTodayByUser/"+id);
  }

  getEventsBeforeJoinGroup(id:number){
    return this.http.get<any>(this.urlApi + "api/Suscripcion/GetEventsTodayByUser/"+id);
  }
}
