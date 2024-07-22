import React from 'react';
import { useForm } from 'react-hook-form';

import { aspectsModel, type AssignedAspect } from '../../../../entities/geoobject';
import { Modal } from '../../../../shared/ui/modal';
import { Button } from '../../../../shared/ui/button';

import styles from './assign-aspect-form.module.css';

type Fields = Omit<AssignedAspect, 'id'>;

export const AssignAspectForm = () => {
    const {
        register,
        handleSubmit,
        formState: { isValid },
        reset,
    } = useForm<Fields>({});

    const handleSave = async (data: Fields) => {
        await aspectsModel.assignAspectFx(data);
        handleClose();
    };

    const handleClose = () => {
        aspectsModel.setIsNewAspectModalOpen(false);
        reset();
    };

    return (
        <Modal onClose={handleClose}>
            <h3 className={styles.title}>Добавление нового аспекта</h3>

            <form className={styles.form} onSubmit={handleSubmit(handleSave)}>
                <div>
                    <label>Код аспекта: </label>
                    <input className={styles.input} type="text" {...register('code', { required: true })} />
                </div>

                <div>
                    <label>Название: </label>
                    <input className={styles.input} type="text" {...register('type', { required: true })} />
                </div>

                <div>
                    <label>Описание: </label>
                    <input className={styles.input} type="text" {...register('commonInfo', { required: true })} />
                </div>

                <div className={styles.btns}>
                    <Button mix={styles.btn} disabled={!isValid}>
                        Создать
                    </Button>
                    <Button
                        mix={styles.btn}
                        onClick={(e) => {
                            e.preventDefault();
                            handleClose();
                        }}
                        color="orange"
                    >
                        Закрыть форму
                    </Button>
                </div>
            </form>
        </Modal>
    );
};