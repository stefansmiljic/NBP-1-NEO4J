import React from 'react';
import ReactDOM from 'react-dom/client';
import './index.css';
import App from './App';
import reportWebVitals from './reportWebVitals';
import { BrowserRouter, RouterProvider, createBrowserRouter } from 'react-router-dom'
import Admin from './Admin';

const router = createBrowserRouter([
  { path: '/', element: <App></App> },
  { path: '/admin', element: <Admin></Admin> }
]);


ReactDOM.createRoot(document.getElementById('root')).render(
  <React.StrictMode>
    <RouterProvider router={router}/>
  </React.StrictMode>
)

reportWebVitals();
