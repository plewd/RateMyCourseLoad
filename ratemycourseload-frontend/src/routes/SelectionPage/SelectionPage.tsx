import { Course } from '@types'
import CourseSelectorInput from '@components/CourseSelectorInput/CourseSelectorInput.tsx'
import { useState } from 'react'
import { Stack, Chip, Button, Divider, LinearProgress } from '@mui/joy'
import SelectedCourseList from '@components/SelectedCourseList.tsx'

interface Props {
    courseOptions: Course[]
}

const maxNumberCourses = 6

export default function SelectionPage({ courseOptions }: Props) {
    const [selectedCourses, setSelectCourses] = useState<Course[]>([])
    const isAtMaxCourses = selectedCourses.length >= maxNumberCourses
    const [responseLoading, setResponseLoading] = useState<boolean>(false)

    const removeCourse = (courseName: string) => {
        setSelectCourses(
            selectedCourses.filter((course) => course.Name !== courseName)
        )
    }

    const creditHours = selectedCourses.reduce(
        (acc, curr) => acc + curr.CreditHours,
        0
    )

    return (
        <>
            <Stack spacing={3}>
                <SelectedCourseList
                    selectedCourses={selectedCourses}
                    showDelete={!responseLoading}
                    removeCourse={removeCourse}
                />

                {selectedCourses.length > 0 && (
                    <Chip>{creditHours} Credit Hours</Chip>
                )}
                {!responseLoading && (
                    <CourseSelectorInput
                        courseOptions={courseOptions.filter(
                            (course) =>
                                !selectedCourses.some(
                                    (selectedCourse) =>
                                        selectedCourse.Name === course.Name
                                )
                        )}
                        disabled={isAtMaxCourses}
                        onSelected={(course) => {
                            setSelectCourses([...selectedCourses, course])
                        }}
                    />
                )}

                {selectedCourses.length > 0 && (
                    <>
                        <Divider />
                        <Button
                            disabled={responseLoading}
                            onClick={() => setResponseLoading(true)}
                        >
                            Rate my course load
                        </Button>
                        {responseLoading && <LinearProgress />}
                    </>
                )}
            </Stack>
        </>
    )
}
