// Em frontend/src/app/services/auth.interceptor.ts
import { HttpEvent, HttpHandlerFn, HttpInterceptorFn, HttpRequest } from '@angular/common/http';
import { Observable } from 'rxjs';

export const authInterceptor: HttpInterceptorFn = (
  req: HttpRequest<unknown>,
  next: HttpHandlerFn
): Observable<HttpEvent<unknown>> => {

  const token = localStorage.getItem('authToken');

  // Se o token existir, clonamos a requisição e adicionamos o header de autorização.
  if (token) {
    const cloned = req.clone({
      headers: req.headers.set('Authorization', `Bearer ${token}`),
    });

    return next(cloned);
  }

  // Se não houver token, apenas continuamos com a requisição original.
  return next(req);
};