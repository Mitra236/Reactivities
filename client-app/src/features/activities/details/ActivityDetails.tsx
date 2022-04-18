import { observer } from 'mobx-react-lite';
import { useEffect } from 'react';
import { useParams } from 'react-router';
import { Grid } from 'semantic-ui-react';
import LoadingComponent from '../../../app/layout/LoadingComponent';
import { useStore } from '../../../app/stores/store';
import ActivityDetailedInfo from './AcitivityDeatiledInfo';
import ActivityDetailedChat from './ActivityDetailedChat';
import ActivityDetailedHeader from './ActivityDetailedHeader';
import ActivityDetailedSideBar from './ActivityDetailedSideBar';

export default observer (function ActivityDetails() {
    
    const {activityStore} = useStore();
    const {selectedActivity: activity, loadActivity, loadingInitial, clearSelectedActivity } = activityStore;
    const {id} = useParams<{id: string}>();

    useEffect(() => {
        if (id) loadActivity(id);
        return () => clearSelectedActivity();
    }, [id, loadActivity, clearSelectedActivity])

    if (loadingInitial || !activity) return <LoadingComponent />;

    return (
        <Grid>
            <Grid.Column width={10}>
                <ActivityDetailedHeader activity={activity} />
                <ActivityDetailedInfo activity={activity}/>
                <ActivityDetailedChat activityId={activity.id} />
            </Grid.Column>
            <Grid.Column>
                <ActivityDetailedSideBar activity={activity}/>
            </Grid.Column>
        </Grid>
    )
})