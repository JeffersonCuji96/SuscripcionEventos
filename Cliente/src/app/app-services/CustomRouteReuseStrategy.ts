import { ComponentRef } from '@angular/core';
import { RouteReuseStrategy, ActivatedRouteSnapshot, DetachedRouteHandle } from '@angular/router';

export class CustomRouteReuseStrategy implements RouteReuseStrategy {

  private storedRoutes = new Map<string, DetachedRouteHandle>();
  //Cuando se sale de un ruta se llama a este método el cual si devuelve TRUE se ejecutara el método store()
  shouldDetach(route: ActivatedRouteSnapshot): boolean {
    const { reuseRoute } = route.data;
    if (reuseRoute) {
      return true;
    }
    return false;
  }
  //En este método realizamos el guardado de las instancias de las rutas que queremos reutilizar
  store(
    route: ActivatedRouteSnapshot,
    handle: DetachedRouteHandle | null
  ): void { 
    if(handle===null){
      return;
    }
    const key=this.generateKey(route);
    this.storedRoutes.set(key,handle);
  }
  //Si ingresamos a una ruta este método se ejecutará y si devuelve TRUE se ejecutara el método retrieve()
  shouldAttach(route: ActivatedRouteSnapshot): boolean {
    return this.storedRoutes.has(this.generateKey(route));
  }
  //Este método retornaria la instancia de la ruta guardada anteriormente, si la instancia no fue guardada retornara un valor NULL
  retrieve(route: ActivatedRouteSnapshot): DetachedRouteHandle | null {
    return this.storedRoutes.get(this.generateKey(route)) ?? null;
  }
  //Se ejecuta cada vez que cambia una ruta, determina si se reutilizará la ruta
  shouldReuseRoute(
    future: ActivatedRouteSnapshot,
    curr: ActivatedRouteSnapshot
  ): boolean {
    const { reuseRoute } = curr.data;
    if (reuseRoute) {
      return false;
    }
    return curr.routeConfig === future.routeConfig;
  }
  //Extrae la url de cada uno de los segmentos de la ruta para devolver la ruta completa
  private generateKey(route: ActivatedRouteSnapshot) {
    const fullPath = route.pathFromRoot.map(node => node.url.join('/'))
      .filter(Boolean)
      .join('/');
    return '/' + fullPath;
  }

  //Destruye un componente asociado
  deleteStoreRoute(url:string):void{
    const handle=this.storedRoutes.get(url);
    if(handle === undefined){
      return;
    }
    (handle as {componentRef:ComponentRef<any>}).componentRef.destroy();
    this.storedRoutes.delete(url);
  }
}
