import {
    Chip,
    List,
    ListItem,
    ListItemButton,
    ListItemContent,
    ListItemDecorator,
} from '@mui/joy'
import SchoolIcon from '@mui/icons-material/School'
import DeleteIcon from '@mui/icons-material/Delete'
import { Course } from '@types'

interface Props {
    selectedCourses: Course[]
    showDelete: boolean
    removeCourse: (course: string) => void
}

export default function SelectedCourseList({
    selectedCourses,
    showDelete,
    removeCourse,
}: Props) {
    return (
        <List>
            {selectedCourses.map((course) => (
                <ListItem key={`${course.Name}-${course.CreditHours}`}>
                    <ListItemButton color="primary" variant="plain">
                        <ListItemDecorator>
                            <SchoolIcon />
                        </ListItemDecorator>
                        <ListItemContent>
                            {course.Name}{' '}
                            <Chip
                                style={{
                                    marginLeft: '10px',
                                    marginBottom: '3px',
                                }}
                            >
                                {course.CreditHours}
                            </Chip>
                        </ListItemContent>
                        {showDelete && (
                            <DeleteIcon
                                onClick={() => removeCourse(course.Name)}
                            />
                        )}
                    </ListItemButton>
                </ListItem>
            ))}
        </List>
    )
}
