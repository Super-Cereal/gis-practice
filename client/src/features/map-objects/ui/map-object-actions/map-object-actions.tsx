import React, { useEffect } from 'react';
import { useUnit } from 'effector-react';

import { Button } from '../../../../shared/ui/button';
import { mapModel } from '../../../../entities/map';
import { classifiersModel, topologyModel, geoObjectModel, type Classifier } from '../../../../entities/geoobject';
import { geoObjectFormModel } from '../../../geoobject-form';

import { MapObjectDescription } from '../map-object-description/map-object-description';
import { mapObjectsModel } from '../../lib/map-objects.model';

import styles from './map-object-actions.module.css';

/** Рендерит возможные действия с геообьектом */
export const MapObjectActions = () => {
    const selectedGeoobject = useUnit(mapObjectsModel.$selectedGeoobject);
    const selectedAspect = useUnit(mapModel.$mapAspect);
    const geoObjects = useUnit(geoObjectModel.$geoObjects);
  

    const parentChildLinks = useUnit(topologyModel.$parentChildLinks);

    useEffect(() => {
<<<<<<< HEAD
        topologyModel.getParentChildLinksFx();
=======
        if (selectedGeoobject) {
            classifiersModel.getClassifiersFx();
            topologyModel.getParentChildLinks();
            classifiersModel.getGeoObjectClassifiersFx(selectedGeoobject.id);
        }
>>>>>>> 3386e292 (Отображение границы между топологиями)
    }, [selectedGeoobject]);

    const childGeoObjects = parentChildLinks
        .filter((link) => link.parentGeographicalObjectId === selectedGeoobject?.id)
        .map((link) => link.childGeographicalObjectId)
        .flatMap((id) => geoObjects.find((geoObject) => geoObject.id === id));

    const parentGeoObjects = parentChildLinks
        .filter((link) => link.childGeographicalObjectId === selectedGeoobject?.id)
        .map((link) => link.parentGeographicalObjectId)
        .flatMap((id) => geoObjects.find((geoObject) => geoObject.id === id));
    
    //from update
    const handleUpdateModalFormOpen = () => {
        geoObjectFormModel.setIsUpdateModalOpen(true);
    };

    //form child
    const handleChildModalFormOpen = () => {
        geoObjectFormModel.setIsChildModalOpen(true);
        console.log(geoObjectFormModel.$isChildModalOpen.getState());
    };

    if (!selectedGeoobject) {
        return <h3>Нажмите на геообьект, чтобы редактировать его</h3>;
    }

    const { id, name } = selectedGeoobject;

    const handleDelete = () => {
        geoObjectModel.deleteGeoObjectFx(id);
    };

    return (
        <div>
            <MapObjectDescription
                parentGeoObjects={parentGeoObjects}
                geoObject={selectedGeoobject}
                childGeoObjects={childGeoObjects}
            /* geoObjectClassifierObjects={geoObjectClassifierObjects} */
            />

            <div className={styles.btns}>
                {selectedAspect ? (
                    <Button /* onClick={handleUpdateModalFormOpen} */>
                        Создать {selectedAspect.type} для геообъекта
                    </Button>
                ) : (
                    <>
                        <Button mix={styles.btn} onClick={handleUpdateModalFormOpen}>
                            Изменить геообъект
                        </Button>

                        <Button mix={styles.btn} onClick={handleChildModalFormOpen}>
                            Создать дочерний геообъект
                        </Button>
                    </>
                )}

                <Button onClick={handleDelete} color="orange">
                    Удалить
                </Button>
            </div>
        </div>
    );
};
