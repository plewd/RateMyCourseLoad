import { Course } from '@types'
import { API_ENDPOINT } from '@configuration'
import {
    Autocomplete,
    Button,
    CircularProgress,
    FormHelperText,
    Grid,
} from '@mui/joy'
import ClassIcon from '@mui/icons-material/Class'
import PublishIcon from '@mui/icons-material/Publish'
import React, { useState } from 'react'
import useWindowDimensions from '../../hooks/useWindowDimensions.ts'

interface Props {
    coursePrefixOptions: string[]
    onSelected: (selected: Course) => void
    disabled: boolean
}

type CourseSuggestion = {
    prefix: string
    number: string
    title: string
    description: string
    creditHours: number
}

const courseSuggestionCache: Record<string, CourseSuggestion[]> = {}

function CourseSelectorInput({
    coursePrefixOptions,
    onSelected,
    disabled,
}: Props) {
    const [coursePrefix, setCoursePrefix] = useState<string | null>(null)
    const [course, setCourse] = useState<CourseSuggestion | null>(null)
    const [loading, setLoading] = useState<boolean>(false)

    const [courseSuggestions, setCourseSuggestions] = useState<
        CourseSuggestion[] | null
    >(null)

    const fetchCourseSuggestions = async (
        coursePrefix: string
    ): Promise<CourseSuggestion[]> => {
        if (coursePrefix in courseSuggestionCache) {
            return courseSuggestionCache[coursePrefix]
        }

        const response = await fetch(
            `${API_ENDPOINT}/course/prefix/${coursePrefix}`
        )
        if (!response.ok) {
            throw new Error('Network response was not ok')
        }

        const courseSuggestions = await response.json()
        courseSuggestionCache[coursePrefix] = courseSuggestions
        return courseSuggestions
    }

    const updateCourseList = async (coursePrefix: string) => {
        try {
            setLoading(true)
            const suggestions: CourseSuggestion[] =
                await fetchCourseSuggestions(coursePrefix)
            setCourseSuggestions(suggestions)
            setLoading(false)
        } catch (error) {
            console.error('Error:', error)
            alert('womp...')
        }
    }

    const coursePrefixSet = (
        _event: React.SyntheticEvent,
        value: string | null
    ) => {
        if (value !== null) {
            setCoursePrefix(value)
            setCourse(null)
            void updateCourseList(value)
        } else {
            setCoursePrefix(null)
            setCourse(null)
        }
    }

    const isCourseReadyToAdd = coursePrefix !== null && course !== null
    const { width } = useWindowDimensions()
    const showIcon = coursePrefix === null || width > 500

    return (
        <Grid container spacing={1}>
            <Grid xs={5}>
                <Autocomplete
                    variant="soft"
                    autoSelect
                    autoHighlight
                    clearOnBlur
                    blurOnSelect
                    onChange={coursePrefixSet}
                    value={coursePrefix}
                    placeholder="Prefix"
                    disabled={disabled}
                    startDecorator={showIcon ? <ClassIcon /> : <></>}
                    options={coursePrefixOptions.sort()}
                />
            </Grid>

            <Grid xs={5}>
                <Autocomplete
                    variant="soft"
                    autoSelect
                    autoHighlight
                    clearOnBlur
                    blurOnSelect
                    onChange={(_event, value) => {
                        if (value !== null) {
                            setCourse(value)
                        }
                    }}
                    value={course}
                    placeholder={loading ? '' : 'Number'}
                    disabled={loading || coursePrefix === null}
                    getOptionLabel={(option) => option.number}
                    options={
                        courseSuggestions === null ? [] : courseSuggestions
                    }
                    endDecorator={
                        loading && (
                            <CircularProgress
                                size="sm"
                                sx={{ bgcolor: 'background.surface' }}
                            />
                        )
                    }
                />
            </Grid>
            <Grid xs={2}>
                <Button
                    disabled={!isCourseReadyToAdd}
                    onClick={() => {
                        if (course !== null) {
                            onSelected({
                                Name: `${course.prefix}${course.number}`,
                                CreditHours: course.creditHours,
                                Title: course.title,
                            })
                            // setCoursePrefix(null)
                            // setCourseSuggestions(null)
                            setCourse(null)
                        }
                    }}
                >
                    <PublishIcon />
                </Button>
            </Grid>
            {isCourseReadyToAdd && (
                <Grid xs={12} style={{ paddingTop: '10px' }}>
                    <FormHelperText>{course.title}</FormHelperText>
                </Grid>
            )}
            {disabled && (
                <FormHelperText style={{ paddingTop: '10px' }}>
                    The maximum amount of courses have been added
                </FormHelperText>
            )}
        </Grid>
    )
}

export default CourseSelectorInput
