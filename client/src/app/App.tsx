import React from 'react';
import { BrowserRouter, Routes, Route } from 'react-router-dom';

import { MainPage } from '../pages/main';
import { ErrorBoundary } from '../shared/lib/error-boundary';
import { Page } from '../shared/ui/page';

import './a11y.css';
import './App.css';

export const App = () => {
    return (
        <React.StrictMode>
            <ErrorBoundary>
                <BrowserRouter basename="/">
                    <Routes>
                        <Route path="/" element={<MainPage />} />
                        <Route path="/main" element={<MainPage />} />
                    </Routes>
                </BrowserRouter>
            </ErrorBoundary>
        </React.StrictMode>
    );
};

const Redirects = ({ children }: React.PropsWithChildren) => {
    if (false) {
        return <Page>Авторизация...</Page>;
    }

    return <>{children}</>;
};